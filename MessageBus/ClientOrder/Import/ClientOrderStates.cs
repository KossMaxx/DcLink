namespace MessageBus.ClientOrder.Import
{
    public enum ClientOrderStates
    {
        WaitAgreement = 1,
        ToEnsure,
        ReadyToEnsure,
        ToShipment,
        WaitEnsure,
        ReadyToShipment,
        InProcessOfShipment,
        AfterShipment,
        ReadyToClose,
        Close,
    }
}