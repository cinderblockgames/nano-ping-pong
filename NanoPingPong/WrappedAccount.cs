using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DockerHax.IO;
using N2.Pow;
using Nano.Net;
using Nano.Net.Extensions;
using NanoPingPong.Shared.Config;

namespace NanoPingPong
{
    public class WrappedAccount
    {

        private RpcClients Clients { get; }
        private WorkServer WorkServer { get; }
        private IContext Context { get; }

        public WrappedAccount(RpcClients clients, WorkServer workServer, IContext context)
        {
            Clients = clients;
            WorkServer = workServer;
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
                            // Can't use await in a try-catch.
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
        }

        public async Task Return(string sender, BigInteger raw)
        {
            Log("Pong");
            var work = await GenerateWork(Context.SendDifficulty);
            var send = Block.CreateSendBlock(Context.Account, sender, new Amount(raw), work);
            await Clients.Node.ProcessAsync(send);
        }

        private async Task<string> GenerateWork(string difficulty)
        {
            await Clients.Node.UpdateAccountAsync(Context.Account);
            var hash = Context.Account.Opened ? Context.Account.Frontier : Context.Account.PublicKey.BytesToHex();
            var response = await WorkServer.GenerateWork(hash);
            if (response.ErrorResult != null)
            {
                Log($"{response.ErrorResult.Error}: {response.ErrorResult.Message}");
            }
            return response.WorkResult?.Work;
        }

        private void Log(params string[] messages)
        {
            try
            {
                File.AppendAllLines(Context.LogFile, messages);
            }
            catch { }
        }

    }
}
