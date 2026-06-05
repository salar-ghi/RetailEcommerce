namespace Domain.Enums;

public enum FinanceCurrency
{
    IRR = 1,
    USD = 2,
    EUR = 3
}

public enum FinanceBranchType
{
    Supermarket = 1,
    ChainStore = 2,
    Hypermarket = 3,
    OnlineStore = 4,
    Warehouse = 5,
    HeadOffice = 6
}

public enum LedgerAccountType
{
    Asset = 1,
    Liability = 2,
    Equity = 3,
    Revenue = 4,
    Expense = 5,
    ContraAsset = 6,
    ContraRevenue = 7
}

public enum LedgerAccountNormalBalance
{
    Debit = 1,
    Credit = 2
}

public enum FinanceAccountType
{
    Cash = 1,
    Bank = 2,
    PaymentGateway = 3,
    DigitalWallet = 4,
    PettyCash = 5,
    Clearing = 6
}

public enum JournalEntryStatus
{
    Draft = 1,
    PendingApproval = 2,
    Posted = 3,
    Reversed = 4,
    Rejected = 5
}

public enum FinanceSourceDocumentType
{
    ManualJournal = 1,
    SalesOrder = 2,
    PurchaseOrder = 3,
    SupplierInvoice = 4,
    CustomerRefund = 5,
    Payroll = 6,
    RecurringBill = 7,
    TaxSettlement = 8,
    BankTransfer = 9,
    InventoryAdjustment = 10
}

public enum FinanceTransactionDirection
{
    Debit = 1,
    Credit = 2
}

public enum FinancialTransactionStatus
{
    Scheduled = 1,
    PendingApproval = 2,
    Approved = 3,
    Completed = 4,
    Rejected = 5,
    Cancelled = 6,
    Reversed = 7
}

public enum FinancePaymentMethod
{
    Cash = 1,
    Card = 2,
    BankTransfer = 3,
    OnlineGateway = 4,
    Cheque = 5,
    Wallet = 6
}

public enum RecurrenceCycle
{
    Weekly = 1,
    Monthly = 2,
    Quarterly = 3,
    Yearly = 4
}

public enum PayrollRunStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    Paid = 4,
    Cancelled = 5
}

public enum ApprovalDecision
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    ReturnedForCorrection = 4
}
