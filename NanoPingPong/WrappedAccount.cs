using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DockerHax.IO;
using Nano.Net;
using Nano.Net.Extensions;

namespace NanoPingPong
{
    public class WrappedAccount
    {

        private Account Account { get; }
        private RpcClients Clients { get; }
        private Context Context { get; }

        public WrappedAccount(Account account, RpcClients clients, Context context)
        {
            Account = account;
            Clients = clients;
            Context = context;
        }

        private static bool _running = false;
        private static readonly object _locker = new();

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
                        catch (Exception ex)
                        {
                            Log(ex.Message);
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
                Log("Ping received.");
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
                    Log(ex.Message);
                }
            }
        }

        private async Task Receive(ReceivableBlock block)
        {
            if (_cache == null)
            {
                await CacheReceiveWork();
            }
            Log("Processing ping.");
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
            Log("Processing pong.");
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
            _cache = new Cache(await GenerateWork(Context.ReceiveDifficulty), false);
            Log("Ready to process ping.");
        }

        private async Task CacheSendWork()
        {
            _cache = new Cache(await GenerateWork(Context.SendDifficulty), true);
            Log("Ready to pong.");
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

        private void Log(params string[] messages)
        {
            try
            {
                File.AppendAllLines(Context.LogFile, messages);
            }
            catch { }
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
