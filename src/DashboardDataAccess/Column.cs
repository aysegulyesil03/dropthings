﻿namespace Dropthings.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq;
    using System.Text;

    public partial class Column
    {
        #region Methods

        public void Detach()
        {
            this._Page = default(EntityRef<Page>);
            this._WidgetZone = default(EntityRef<WidgetZone>);
        }

        #endregion Methods
    }
}