## ADDED Requirements

### Requirement: View Cart
The system SHALL allow an authenticated user to view their current cart contents.

#### Scenario: Cart retrieved successfully
- **WHEN** an authenticated user requests their cart
- **THEN** the system returns HTTP 200 with all cart items, quantities, and line-item totals

#### Scenario: Empty cart returns empty list
- **WHEN** a user with no cart items requests their cart
- **THEN** the system returns HTTP 200 with an empty items array (not 404)

---

### Requirement: Add Item to Cart
The system SHALL allow an authenticated user to add a product to their cart.

#### Scenario: New item added
- **WHEN** a user adds a product ID and quantity that is not already in their cart
- **THEN** the system creates a new cart item and returns HTTP 200 or 201

#### Scenario: Existing item quantity updated
- **WHEN** a user adds a product ID that already exists in their cart
- **THEN** the system increments the quantity rather than creating a duplicate

#### Scenario: Out-of-stock product rejected
- **WHEN** a user adds a product with quantity exceeding available stock
- **THEN** the system returns HTTP 400

---

### Requirement: Update Cart Item Quantity
The system SHALL allow a user to change the quantity of an existing cart item.

#### Scenario: Quantity updated
- **WHEN** a user sends a valid updated quantity for a cart item
- **THEN** the system returns HTTP 200 with the updated cart item

#### Scenario: Zero quantity removes item
- **WHEN** a user sets quantity to 0
- **THEN** the item is removed from the cart

---

### Requirement: Remove Cart Item
The system SHALL allow a user to remove a specific item from their cart.

#### Scenario: Item removed
- **WHEN** a user sends DELETE for a cart item they own
- **THEN** the system returns HTTP 204 and the item no longer appears in the cart

#### Scenario: Cannot remove another user's cart item
- **WHEN** a user attempts to delete a cart item belonging to another user
- **THEN** the system returns HTTP 403 or HTTP 404
