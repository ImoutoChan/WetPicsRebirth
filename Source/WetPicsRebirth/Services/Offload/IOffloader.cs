namespace WetPicsRebirth.Services.Offload;

public interface IOffloader<in T>
{
    Task Offload(T vote);
}
