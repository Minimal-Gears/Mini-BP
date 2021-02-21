#if DEBUG
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BPMS.Infrastructures.Helper
{
    public class LogCommandInterceptor : DbCommandInterceptor
    {
        private static void WriteLine(CommandEventData data)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($@"EF {data}", "LocalDB");
            Console.ResetColor();
        }

        public override void CommandFailed(DbCommand command, CommandErrorEventData data)
        {
            WriteLine(data);
        }

        public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData data, CancellationToken cancellation)
        {
            WriteLine(data);
            return Task.CompletedTask;
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData data, DbDataReader result)
        {
            WriteLine(data);
            return result;
        }

        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData data, object result)
        {
            WriteLine(data);
            return result;
        }

        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData data, int result)
        {
            WriteLine(data);
            return result;
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            WriteLine(eventData);
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
        {
            WriteLine(eventData);
            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            WriteLine(eventData);
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }
    }
}
#endif
