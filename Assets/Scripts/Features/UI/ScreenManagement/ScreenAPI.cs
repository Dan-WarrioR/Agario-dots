using DG.Tweening;
using Unity.Entities;

namespace Features.UI.ScreenManagement
{
    public class ScreenAPI
    {
        public static T OpenScreen<T>(World world, TweenCallback callback = null) where T : BaseScreen
        {
            return GetSystem(world).OpenScreen<T>(callback);
        }

        public static void CloseScreen<T>(World world, TweenCallback callback = null) where T : BaseScreen
        {
            GetSystem(world).CloseScreen<T>(callback);
        }

        public static TChild OpenSubScreen<TChild, TParent>(World world, TweenCallback callback = null)
            where TChild : BaseScreen
            where TParent : BaseScreen
        {
            return GetSystem(world).OpenSubScreen<TChild, TParent>(callback);
        }

        public static void CloseSubScreen<TChild, TParent>(World world, TweenCallback callback = null)
            where TChild : BaseScreen
            where TParent : BaseScreen
        {
            GetSystem(world).CloseSubScreen<TChild, TParent>(callback);
        }

        private static ScreenSystem GetSystem(World world)
        {
            return world.GetExistingSystemManaged<ScreenSystem>();
        }
    }
}