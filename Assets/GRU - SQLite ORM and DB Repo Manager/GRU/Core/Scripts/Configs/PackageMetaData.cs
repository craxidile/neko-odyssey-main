using System;

namespace SpatiumInteractive.Libraries.Unity.GRU.Configs.General
{
    [Serializable]
    public class PackageMetaData
    {
        #region Public Fields

        public bool HasSetPaths;
        public string PackageFolderLocation;
        public string IntegrationFolderLocation;

        #endregion

        #region Constructors

        public PackageMetaData
        (
            bool hasSetPaths,
            string packageFolderLocation,
            string integrationFolderLocation
        )
        {
            HasSetPaths = hasSetPaths;
            PackageFolderLocation = packageFolderLocation;
            IntegrationFolderLocation = integrationFolderLocation;
        }

        #endregion
    }
}
