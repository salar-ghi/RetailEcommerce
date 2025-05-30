namespace Domain.Enums;

public enum SupplierStatus
{
    Pending, // Awaiting approval
    Approved,  // Active and Approved by admin
    Inactive, // Temporarily inactive, can be reactivated
    Rejected, // Rejected by admin, cannot be reactivated
}
