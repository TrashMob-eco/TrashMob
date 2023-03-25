namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading.Tasks;
    using TrashMob.Poco;

    public interface IImageManager
    {
        public Task UploadImage(ImageUpload imageUpload);
    }
}
