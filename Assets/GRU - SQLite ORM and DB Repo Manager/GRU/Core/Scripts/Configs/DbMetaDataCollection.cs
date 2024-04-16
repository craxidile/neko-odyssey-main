using System;
using System.Collections.Generic;

namespace SpatiumInteractive.Libraries.Unity.GRU.Configs.General
{
    [Serializable]
    public class DbMetaDataCollection
    {
        #region Public Fields

        public List<DbMetaData> DbMetaData;

        #endregion

        #region Constructors

        public DbMetaDataCollection()
        {
            DbMetaData = new List<DbMetaData>();
        }

        public DbMetaDataCollection(List<DbMetaData> dbMetaData)
        {
            DbMetaData = dbMetaData;
        }

        #endregion
    }
}
