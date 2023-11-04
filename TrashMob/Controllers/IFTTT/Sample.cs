namespace TrashMob.Controllers.IFTTT
{
    public class Sample
    {
        public IftttTrigger triggers { get; set; }
        
        public IftttTriggerFieldValidation triggerFieldValidations { get; set; }

        public IftttAction actions { get; set; }

        public IftttActionRecordSkipping actionRecordSkipping { get; set; }

        public Sample()
        {
            triggers = new IftttTrigger();
            actions = new IftttAction();
            actionRecordSkipping = new IftttActionRecordSkipping();
            triggerFieldValidations = new IftttTriggerFieldValidation();
        }
    }
}
