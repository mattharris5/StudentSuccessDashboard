using System;

namespace SSD.ViewModels
{
    public interface IStateCopier<TModel>
    {
        void CopyTo(TModel model);
        void CopyFrom(TModel model);
    }
}
