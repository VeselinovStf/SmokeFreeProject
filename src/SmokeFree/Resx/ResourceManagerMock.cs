using System.Resources;

namespace SmokeFree.Resx
{
    /// <summary>
    /// Unit Test Resource Manager Wrapper
    /// </summary>
    public static class ResourceManagerMock
    {
        /// <summary>
        /// Get Resource Manager
        /// </summary>
        /// <returns>Application Resource Manager</returns>
        public static ResourceManager GetResourceManager()
        {
            return AppResources.ResourceManager;
        }

    }
}
