namespace Framework.Notifications.Manager
{
    public enum NotificationDeliveryResultType
    {
        OK = 1,
        SendError = 2,
        MessageValidationErrors = 3,
        ConfigurationValidationError = 4,
        Cancelado = 5
    }
}
