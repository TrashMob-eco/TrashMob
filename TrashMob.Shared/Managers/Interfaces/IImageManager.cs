namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco;

    public interface IImageManager
    {
        public Task UploadImage(ImageUpload imageUpload);
    }
}
