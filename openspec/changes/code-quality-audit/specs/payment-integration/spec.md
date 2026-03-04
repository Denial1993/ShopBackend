## ADDED Requirements

### Requirement: Initiate Payment
The system SHALL generate an ECPay payment form for an existing Pending order.

#### Scenario: Payment form generated successfully
- **WHEN** an authenticated user requests payment for an order in "Pending" status
- **THEN** the system returns HTTP 200 with the ECPay HTML form or redirect URL

#### Scenario: Paid order cannot be re-initiated
- **WHEN** a user requests payment for an order already in "Paid" status
- **THEN** the system returns HTTP 400

#### Scenario: Another user's order rejected
- **WHEN** a user requests payment for an order they do not own
- **THEN** the system returns HTTP 403

---

### Requirement: ECPay Callback Processing
The system SHALL handle the asynchronous callback (ReturnURL) from ECPay after a payment attempt.

#### Scenario: Successful payment callback
- **WHEN** ECPay sends a POST callback with RtnCode=1 (success) for a valid order
- **THEN** the system updates the order status to "Paid" and returns HTTP 200 with "1|OK"

#### Scenario: Failed payment callback
- **WHEN** ECPay sends a POST callback with RtnCode != 1
- **THEN** the system records the failure and the order status remains "Pending"

#### Scenario: Invalid or tampered callback rejected
- **WHEN** the callback CheckMacValue does not match the computed hash
- **THEN** the system rejects the callback and returns a non-OK response
