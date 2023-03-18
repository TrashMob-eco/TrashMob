namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading.Tasks;
    using TrashMob.Poco;

    public interface IImageManager
    {
        public Task<bool> UpdloadImage(ImageUpload imageUpload);

    }
}
