using BPM.Maps;
//using BPM.Maps.Interfaces;
using BPM.Maps.Implementations;
using BPM.Maps.Interfaces;
using BPM.Repositories.DataContext;
using BPM.Repositories.Implementations;
using BPM.Repositories.Interfaces;
using BPM.Services;
//using BPM.Services.Interfaces;
using BPM.Services.Implementations;
using BPM.Services.Interfaces;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using System.Web;


namespace BPM.DependencyInjection
{
 
    public static class UnityConfig
    {
        private static UnityContainer container;

        public static UnityContainer Container { get { return container; } }

        public static UnityContainer RegisterComponents()
        {
            container = new UnityContainer();
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<FrameworkEntities>();
            container.RegisterType<IFrameworkEntities, FrameworkEntities>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuthenticationManager>(new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));
           // container.RegisterType<SisUsuario>(new InjectionConstructor(typeof(FrameworkEntities)));

            #region Mappers

            container.RegisterType<IUserMap, UserMap>();
            container.RegisterType<IRoleMap, RoleMap>();
          

            #endregion

            #region Services

          container.RegisterType<IUserService, UserService>();
          container.RegisterType<IRolService, RolService>();
          container.RegisterType<IListaPermisoService, ListaPermisoService>();
           
            #endregion

            #region Repositories

            container.RegisterType<IUsuarioRepository, UsuarioRepository>();
            container.RegisterType<IRolRepository, RolRepository>();
            container.RegisterType<IListaPermisoRepository, ListaPermisoRepository>();
      
            #endregion

            return container;
        }

        public static T ResolveDependency<T>()
        {
            if (container == null)
            {
                container = new UnityContainer();
                RegisterComponents();
            }
            return container.Resolve<T>();
        }
    }
}
