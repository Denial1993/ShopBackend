## ADDED Requirements

### Requirement: List Products
The system SHALL allow any user (authenticated or anonymous) to retrieve a list of available products.

#### Scenario: Successful product listing
- **WHEN** a GET request is made to the products endpoint
- **THEN** the system returns HTTP 200 with an array of product objects including id, name, price, and image URL

---

### Requirement: Get Single Product
The system SHALL return detailed information for a specific product by its ID.

#### Scenario: Existing product found
- **WHEN** a GET request includes a valid product ID
- **THEN** the system returns HTTP 200 with the full product details

#### Scenario: Non-existent product
- **WHEN** a GET request includes an ID that does not exist
- **THEN** the system returns HTTP 404

---

### Requirement: Admin Product Creation
The system SHALL allow only admin users to create new products.

#### Scenario: Admin creates product successfully
- **WHEN** an authenticated admin submits valid product data (name, price, stock)
- **THEN** the system returns HTTP 201 with the created product

#### Scenario: Non-admin creation rejected
- **WHEN** a non-admin authenticated user attempts to create a product
- **THEN** the system returns HTTP 403

#### Scenario: Missing required fields rejected
- **WHEN** admin submits product data with missing required fields
- **THEN** the system returns HTTP 400 with field-level validation errors

---

### Requirement: Admin Product Update
The system SHALL allow only admin users to update existing product information, including uploading a product image.

#### Scenario: Admin updates product
- **WHEN** an admin submits valid updated product data for an existing ID
- **THEN** the system returns HTTP 200 with the updated product

#### Scenario: Image upload succeeds
- **WHEN** an admin uploads an image file during product update
- **THEN** the image is stored (via Supabase storage) and the product's image URL is updated

---

### Requirement: Admin Product Deletion
The system SHALL allow only admin users to delete products.

#### Scenario: Admin deletes product
- **WHEN** an admin sends DELETE to an existing product ID
- **THEN** the system returns HTTP 204 and the product is no longer retrievable

#### Scenario: Non-admin deletion rejected
- **WHEN** a non-admin attempts to delete a product
- **THEN** the system returns HTTP 403
