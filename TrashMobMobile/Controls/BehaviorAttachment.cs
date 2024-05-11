namespace TrashMobMobile.Controls;

public static class BehaviorAttachment
{
    public static readonly BindableProperty AttachBehaviorProperty =
        BindableProperty.CreateAttached(
            "PassthroughBehavior",
            typeof(Behavior),
            typeof(BehaviorAttachment),
            null,
            propertyChanged: OnBehaviorChanged);

    public static void SetAttachBehavior(BindableObject view, Behavior value)
    {
        view.SetValue(AttachBehaviorProperty, value);
    }

    public static Behavior GetAttachBehavior(BindableObject view)
    {
        return (Behavior)view.GetValue(AttachBehaviorProperty);
    }

    static void OnBehaviorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is Behavior newBehavior)
        {
            if (bindable is TMEntry tmEntry)
            {
                var entry = tmEntry.FindByName<Entry>("WrappedEntry");
                if (entry != null)
                {
                    entry.Behaviors.Add(newBehavior);
                }
            }

            if (bindable is TMEditor tmEditor)
            {
                var editor = tmEditor.FindByName<Editor>("WrappedEditor");
                if (editor != null)
                {
                    editor.Behaviors.Add(newBehavior);
                }
            }
        }        
    }
}

