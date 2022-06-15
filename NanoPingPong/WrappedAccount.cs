using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Nano.Net;
using Nano.Net.Extensions;
using static NanoPingPong.Constants;

namespace NanoPingPong
{
    public class WrappedAccount
    {

        private Account Account { get; }
        private RpcClients Clients { get; }

        public WrappedAccount(Account account, RpcClients clients)
        {
            Account = account;
            Clients = clients;
        }

        private static bool _running = false;
        private static object _locker = new();

        public void Tick()
        {
            if (!_running)
            {
                lock (_locker)
                {
                    if (!_running)
                    {
                        _running = true;
                        try
                        {
                            Process().GetAwaiter().GetResult();
                        }
                        finally
                        {
                            _running = false;
                        }
                    }
                }
            }

        }

        private Cache _cache;

        private async Task Process()
        {
            if (_cache == null || !_cache.Sendable)
            {
                await CacheSendWork();
            }

            var pending = await Clients.Node.PendingBlocksAsync(Account.Address, int.MaxValue);
            var blocks = pending?.PendingBlocks?.Select(block => block.Value) ?? Enumerable.Empty<ReceivableBlock>();
            foreach (var block in blocks)
            {
                try
                {
                    if (Amount.FromRaw(block.Amount).Nano <= 1)
                    {
                        await Return(block.Source, BigInteger.Parse(block.Amount));
                        await Receive(block);
                    }
                    else
                    {
                        await Receive(block);
                        await Return(block.Source, BigInteger.Parse(block.Amount));
                    }
                    if (_cache == null || !_cache.Sendable)
                    {
                        await CacheSendWork();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private async Task Receive(ReceivableBlock block)
        {
            if (_cache == null)
            {
                await CacheReceiveWork();
            }
            var receive = Block.CreateReceiveBlock(Account, block, _cache.Work);
            await Clients.Node.ProcessAsync(receive);
            await CacheSendWork();
        }

        public async Task Return(string sender, BigInteger nano)
        {
            if (_cache == null || !_cache.Sendable)
            {
                await CacheSendWork();
            }
            var send = Block.CreateSendBlock(
                Account,
                sender,
                new Amount(nano),
                _cache.Work);
            await Clients.Node.ProcessAsync(send);
            await CacheReceiveWork();
        }

        private async Task CacheReceiveWork()
        {
            _cache = new Cache(await GenerateWork(Difficulty.Receive), false);
        }

        private async Task CacheSendWork()
        {
            _cache = new Cache(await GenerateWork(Difficulty.Send), true);
        }

        private async Task<string> GenerateWork(string difficulty)
        {
            await Clients.Node.UpdateAccountAsync(Account);
            var work = await Clients.WorkServer.WorkGenerateAsync(
                Account.Opened ? Account.Frontier : Account.PublicKey.BytesToHex(),
                difficulty
            );
            return work?.Work;
        }

        private class Cache
        {
            public string Work { get; }
            public bool Sendable { get; }
            public Cache(string work, bool sendable)
            {
                Work = work;
                Sendable = sendable;
            }
        }

    }
}
