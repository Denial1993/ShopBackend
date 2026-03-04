## ADDED Requirements

### Requirement: Create Order from Cart
The system SHALL allow an authenticated user to create an order based on their current cart items.

#### Scenario: Successful order creation
- **WHEN** an authenticated user submits a checkout request with a non-empty cart
- **THEN** the system creates an order with status "Pending", returns HTTP 201, and the cart is cleared

#### Scenario: Empty cart rejected
- **WHEN** a user attempts to checkout with an empty cart
- **THEN** the system returns HTTP 400

---

### Requirement: View Own Orders
The system SHALL allow an authenticated user to retrieve only their own orders.

#### Scenario: User views their orders
- **WHEN** an authenticated user requests their order list
- **THEN** the system returns HTTP 200 with only orders belonging to that user

#### Scenario: View single order
- **WHEN** an authenticated user requests an order by ID that belongs to them
- **THEN** the system returns HTTP 200 with full order details including line items

#### Scenario: Access to another user's order denied
- **WHEN** an authenticated user requests an order ID belonging to a different user
- **THEN** the system returns HTTP 403 or HTTP 404

---

### Requirement: Order Status Transitions
The system SHALL manage order status through defined transitions.

#### Scenario: Order moves from Pending to Paid after payment
- **WHEN** the payment callback confirms a successful payment for an order
- **THEN** the order status MUST change to "Paid"

#### Scenario: Admin can cancel an order
- **WHEN** an admin sends a cancel request for an existing order
- **THEN** the order status changes to "Cancelled" and HTTP 200 is returned
