using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Interfaces
{
    public interface IFileProviderService<TModel>
    {
        void WriteToDisck(List<TModel> models);
        List<TModel> ReadFromDisck();
    }
}
