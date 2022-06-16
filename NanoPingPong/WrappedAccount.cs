using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DockerHax.IO;
using Nano.Net;
using Nano.Net.Extensions;
using NanoPingPong.Shared.Config;

namespace NanoPingPong
{
    public class WrappedAccount
    {

        private RpcClients Clients { get; }
        private IContext Context { get; }

        public WrappedAccount(RpcClients clients, IContext context)
        {
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
                            // Nano requires a more powerful work server, so add some caching to help it along.
                            // I believe transactions won't need PoW eventually, so this will simplify then.
                            var process = (Context.Banano || Context.CacheWork) ? Process() : ProcessWithCaching();
                            process.GetAwaiter().GetResult();
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

        private async Task Process()
        {
            var pending = await Clients.Node.PendingBlocksAsync(Context.Account.Address, int.MaxValue);
            var blocks = pending?.PendingBlocks?.Select(block => block.Value) ?? Enumerable.Empty<ReceivableBlock>();
            foreach (var block in blocks)
            {
                try
                {
                    await Receive(block);
                    await Return(block.Source, BigInteger.Parse(block.Amount));
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }
            }
        }

        private async Task Receive(ReceivableBlock block)
        {
            Log("Ping");
            var work = await GenerateWork(Context.ReceiveDifficulty);
            var receive = Block.CreateReceiveBlock(Context.Account, block, work);
            await Clients.Node.ProcessAsync(receive);
            await CacheSendWork();
        }

        public async Task Return(string sender, BigInteger raw)
        {
            Log("Pong");
            var work = await GenerateWork(Context.SendDifficulty);
            var send = Block.CreateSendBlock(Context.Account, sender, new Amount(raw), work);
            await Clients.Node.ProcessAsync(send);
            await CacheReceiveWork();
        }

        private async Task<string> GenerateWork(string difficulty)
        {
            await Clients.Node.UpdateAccountAsync(Context.Account);
            var work = await Clients.WorkServer.WorkGenerateAsync(
                Context.Account.Opened ? Context.Account.Frontier : Context.Account.PublicKey.BytesToHex(),
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

        #region " Cached "

        private Cache _cache;

        private async Task ProcessWithCaching()
        {
            if (_cache == null || !_cache.Sendable)
            {
                await CacheSendWork();
            }

            var pending = await Clients.Node.PendingBlocksAsync(Context.Account.Address, int.MaxValue);
            var blocks = pending?.PendingBlocks?.Select(block => block.Value) ?? Enumerable.Empty<ReceivableBlock>();
            foreach (var block in blocks)
            {
                Log("Ping received.");
                try
                {
                    if (Amount.FromRaw(block.Amount).Nano <= 1)
                    {
                        await ReturnWithCaching(block.Source, BigInteger.Parse(block.Amount));
                        await ReceiveWithCaching(block);
                    }
                    else
                    {
                        await ReceiveWithCaching(block);
                        await ReturnWithCaching(block.Source, BigInteger.Parse(block.Amount));
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

        private async Task ReceiveWithCaching(ReceivableBlock block)
        {
            if (_cache == null)
            {
                await CacheReceiveWork();
            }
            Log("Processing ping.");
            var receive = Block.CreateReceiveBlock(Context.Account, block, _cache.Work);
            await Clients.Node.ProcessAsync(receive);
            await CacheSendWork();
        }

        public async Task ReturnWithCaching(string sender, BigInteger raw)
        {
            if (_cache == null || !_cache.Sendable)
            {
                await CacheSendWork();
            }
            Log("Processing pong.");
            var send = Block.CreateSendBlock(
                Context.Account,
                sender,
                new Amount(raw),
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

        #endregion

    }
}
