using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Infrastructure;

public interface IEngineFactory
{
    IPopularListLoaderEngine Get(ImageSource imageSource);
}
