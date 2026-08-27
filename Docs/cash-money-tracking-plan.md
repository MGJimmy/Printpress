# Cash, credit/debit, and money tracking — implementation plan

We implement this together, step by step. This file is the backlog: original vault gaps, loans with installment repayment, and cash reports that show every riyal in and out.

---

## A. Current model (what we have)

Single-entry vaults, not double-entry.

- Two accounts: Main vault, Spare-parts vault.
- Each movement: In or Out, positive amount, category, optional `ReferenceType` + `ReferenceId`.
- Balance is stored on the account and mutated on each transaction.

Automatic In/Out today:

| Event | Vault | Direction | Category |
|--------|--------|-----------|----------|
| Order payment | Main | In | Sales |
| Order refund | Main | Out | Sales |
| Inventory purchase invoice | Main | Out | Purchases (full amount immediately) |
| Spare purchase | SpareParts | Out | Purchases |
| Spare sale | SpareParts | In | Sales |
| Salary / bonus / advance / adjustment | Main | Out | Salaries (no worker id on cash row) |
| Advance repayment | Main | In | Salaries |
| Penalty | — | skipped | — |
| Manual vault movement | chosen | In/Out | limited UI categories |

Does not touch cash: order create, inventory stock-out without invoice, spare stock-out without selling invoice, payroll period close, **deleting a salary transaction**.

---

## B. Original gaps to fix (keep)

1. No receivables / payables (AR/AP). Purchases always cash-out in full at invoice time.
2. Refunds categorized as Sales (cannot separate revenue from returns).
3. Salary delete does not reverse cash.
4. Salary cash rows are not linked to the worker.
5. Out can make balance negative (no insufficient-funds check).
6. Stored balance race (no concurrency token).
7. Two write paths (domain `AddTransaction` vs manual direct `Balance` mutate).
8. No edit / void / delete of cash rows; no Main ↔ SpareParts transfer.
9. Automatic posts ignore business date (`DateTime.UtcNow`).
10. No reconcile (stored balance vs sum of transactions).
11. UI filters miss Salaries; ExternalServices picker labeled as group but binds OrderId.
12. Account types only Main / SpareParts (no bank, drawer, petty cash).
13. Penalty has no cash story (document as rule or post cash if needed).
14. Cash APIs are `[AllowAnonymous]`.
15. Copy-paste description on spare-part selling still says purchase; `CachAccount` typo; EN loc incomplete; account DTO omits Type.

Near-term hardening (do before / with new features):

- One write path for every cash change.
- Block overdraft (or confirm + reason).
- Rowversion on `CashAccount`.
- Void / reversing entry instead of silent delete.
- Salary delete/edit reverses cash; `ReferenceType` + worker id.
- Honor `TransactionDate` from the source document.
- Category `SalesReturn` (or equivalent) for refunds.
- Reconcile screen: stored vs `SUM(In) - SUM(Out)`.
- Transfer between vaults in one DB transaction.
- Auth on cash APIs.
- Show account type and Salaries in UI filters.

Later (full credit/debit):

- Chart of accounts + double-entry journal.
- Client receivable (order vs cash received, statements, aging).
- Supplier payable (invoice vs cash paid, partial pay, due dates).
- Payment methods (cash / bank / cheque).
- Opening balances, period close, voucher numbers.
- Reports: trial balance, P&L, customer aging.

---

## C. New: loans from a person or bank (pay in chunks)

**Business rule**

- We borrow from a lender (person or bank). Record who we borrowed from.
- When we **receive** the loan: cash in the chosen vault **increases**; remaining debt = principal (minus any fees if we model them later).
- When we **repay a chunk**: cash in the vault **decreases**; remaining debt decreases by that payment.
- We never lose the link: each cash In/Out for a loan points at that loan (and optionally that installment).

**Suggested domain (to implement when we reach this step)**

- `Lender` (or `LoanParty`): name, type (Person / Bank), phone, notes. Reuse if we borrow more than once from the same party.
- `Loan`:
  - LenderId
  - CashAccountId (which vault received the money)
  - Principal amount
  - Optional: interest rate, start date, due date, notes
  - Status: Active / FullyPaid / Cancelled
  - RemainingBalance (or derived from payments; prefer derived + stored with reconcile)
- `LoanPayment` (installment / chunk):
  - LoanId
  - Amount
  - PaymentDate
  - Notes
  - CashTransactionId (the Out row)

**Cash posting**

| Event | Cash | Category (new) | Reference |
|--------|------|----------------|-----------|
| Disburse / take loan | In | `LoanReceived` | Loan |
| Repay chunk | Out | `LoanRepayment` | Loan (and LoanPayment id) |

Same rules as other cash: one write path, no silent delete (void reverses both cash and remaining loan), honor payment date, no overdraft unless allowed.

**UI**

- Lenders list (the people/banks we owe).
- Loan: create (amount, lender, vault, date) → posts cash In.
- Loan view: principal, paid, remaining, payment history.
- Add payment: amount ≤ remaining, choose vault (usually same as received), posts cash Out.
- Cannot overpay remaining; cannot delete a paid loan without voiding payments.

**Reports that belong with loans**

- Loans outstanding (by lender).
- Loan statement (one loan: receive + each chunk).
- Payments due / overdue if we store due dates.
- Cash In from loans vs cash Out to repay (period).

This is **payable to a lender**, separate from supplier AP and from owner capital injection.

---

## D. New: all cash reports (every small amount in and out)

Goal: see **where money came from and where it went**, down to the single transaction, then roll up.

Use `TransactionDate`, category, type, account, reference (order / invoice / worker / loan), user, description. Running balance on the cashbook is required.

### 1. Cashbook (دفتر الخزنة) — the core

- Filter: account (or all), date from/to, In/Out, category, amount min/max, free-text description, reference type/id.
- Columns: date/time, account, In, Out, running balance, category, description, linked document, created by.
- Opening balance at start of period + closing at end.
- Print / export Excel.

This is the “very small money” view: every row.

### 2. Movement summary (where it comes / goes)

- Totals In and Out **by category** for a period (Sales, Purchases, Salaries, Loans, …).
- Same by **account** (Main vs SpareParts vs later Bank).
- Same by **day** (daily cash flow).
- Same by **month**.
- Net = In − Out per slice.

### 3. Source / destination drill-down

From a category total, open the list of transactions. From a transaction, open the order, purchase invoice, salary row, or loan.

Needed so “Purchases 12,400” is not a black box.

### 4. Document-linked cash

- Cash per **order** (payments + refunds).
- Cash per **purchase invoice** / spare invoice.
- Cash per **worker** (salary movements).
- Cash per **loan** / **lender**.

### 5. Reconcile

- Stored account balance vs sum of all transactions (all time).
- Period: opening (from txs before from-date) vs txs in period vs closing.
- Flag mismatch.

### 6. Transfers and loans (when those exist)

- Transfer register (from account → to account).
- Loan received vs repaid in period; remaining debt.

### 7. Operational snapshots

- Current balances of all cash accounts (treasury dashboard).
- Largest Out / In in the period.
- Manual vs automatic movements (if we flag source).

### 8. Later (after AR/AP)

- Client statement / aging (not vault-only).
- Supplier unpaid / partial pay.
- P&L and trial balance (needs GL).

**UI placement:** under existing Reports, plus a “كشف حساب الخزنة” button on each cash account screen (pre-filtered).

**Print:** reuse report viewer where invoices already print.

---

## E. Suggested implementation order

We confirm each slice before coding the next.

1. **Harden vault write path** — single `AddTransaction`, concurrency, overdraft, auth, honor date, refund category, salary reverse + worker ref, Salaries in UI filter.
2. **Void / reverse** — no silent delete; reversing cash row.
3. **Reconcile + cashbook report** — first report; proves every riyal is listed.
4. **Summary reports** — by category, day, month, account; drill-down.
5. **Transfers** between vaults.
6. **Loans** — lender, loan, chunk payments, cash In/Out, loan reports.
7. **Client receivables** (order vs paid, client statement).
8. **Supplier payables** (credit invoice + partial pay).
9. Extra account types (bank / drawer) if needed.
10. Double-entry GL only if official books are required.

---

## F. Categories to add when we implement

- `SalesReturn` — order refunds
- `LoanReceived` — taking a loan (cash In)
- `LoanRepayment` — paying a chunk (cash Out)
- Keep `CapitalInjection` for owner money, not loans

---

When we start coding, begin at **E.1** unless we agree to jump to cashbook reports or loans first.
