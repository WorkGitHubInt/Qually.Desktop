using Ninject;

namespace BotQually
{
    public static class IoC
    {
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        public static void Setup()
        {
            BindViewModels();
        }

        private static void BindViewModels()
        {
            Kernel.Bind<MainViewModel>().ToConstant(new MainViewModel());
            Kernel.Bind<GlobalSettings>().ToConstant(new GlobalSettings());
        }
    }
}
