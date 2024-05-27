namespace TrashMob.Controllers.IFTTT
{
    public class Sample
    {
        public Sample()
        {
            triggers = new IftttTrigger();
            actions = new IftttAction();
            actionRecordSkipping = new IftttActionRecordSkipping();
            triggerFieldValidations = new IftttTriggerFieldValidation();
        }

        public IftttTrigger triggers { get; set; }

        public IftttTriggerFieldValidation triggerFieldValidations { get; set; }

        public IftttAction actions { get; set; }

        public IftttActionRecordSkipping actionRecordSkipping { get; set; }
    }
}