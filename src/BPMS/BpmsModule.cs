using System;
using System.Reflection;
using Autofac;
using BPMS.Infrastructures;
using BPMS.Infrastructures.DataAccess;
using BPMS.Infrastructures.DataAccess.Postgres;
using BPMS.Infrastructures.Helper;
using BPMS.Services.CartableService;
using Microsoft.EntityFrameworkCore;
using Module = Autofac.Module;

namespace BPMS
{
    public sealed class BpmsModule : Module
    {
        private readonly string connectionString;

        public BpmsModule(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(a => new PostgresBpmsDbContextOld(connectionString)).As<BpmsDbContext>()
                .InstancePerLifetimeScope();
            builder.RegisterType<PostgresExceptionHelper>().As<IDbExceptionHelper>().InstancePerLifetimeScope();
            builder.RegisterType<BpmsUnitOfWork>().As<IBpmsUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<CartableService>().AsSelf().InstancePerLifetimeScope();
            //  builder.RegisterGeneric(typeof(CartableService<>)).AsSelf().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}