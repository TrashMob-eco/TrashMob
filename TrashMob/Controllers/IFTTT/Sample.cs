namespace TrashMob.Controllers.IFTTT
{
    public class Sample
    {
        public IftttTrigger triggers { get; set; } = new IftttTrigger();
        
        public IftttTriggerFieldValidation triggerFieldValidations { get; set; } = new IftttTriggerFieldValidation();

        public IftttAction actions { get; set; } = new IftttAction();

        public IftttActionRecordSkipping actionRecordSkipping { get; set; } = new IftttActionRecordSkipping();
    }
}
