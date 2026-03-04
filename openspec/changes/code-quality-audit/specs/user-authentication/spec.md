## ADDED Requirements

### Requirement: User Registration
The system SHALL allow a new user to register with a unique email and a password that meets minimum security requirements.

#### Scenario: Successful registration
- **WHEN** a user submits valid email and password
- **THEN** the system creates the account, returns HTTP 201, and does NOT return the password

#### Scenario: Duplicate email rejected
- **WHEN** a user submits an email that already exists
- **THEN** the system returns HTTP 400 or HTTP 409 with an error message

#### Scenario: Weak password rejected
- **WHEN** a user submits a password shorter than 6 characters
- **THEN** the system returns HTTP 400 with a validation error

---

### Requirement: User Login
The system SHALL authenticate a user by email and password and return a signed JWT token.

#### Scenario: Successful login
- **WHEN** a user submits correct email and password
- **THEN** the system returns HTTP 200 with a JWT bearer token

#### Scenario: Wrong password rejected
- **WHEN** a user submits incorrect password
- **THEN** the system returns HTTP 401

#### Scenario: Non-existent user rejected
- **WHEN** a user submits an email not in the database
- **THEN** the system returns HTTP 401 (not 404, to avoid user enumeration)

---

### Requirement: Protected Route Access
The system SHALL restrict access to authenticated endpoints using JWT bearer token validation.

#### Scenario: Valid token grants access
- **WHEN** a request includes a valid, non-expired Authorization: Bearer token
- **THEN** the endpoint processes the request normally

#### Scenario: Missing token rejected
- **WHEN** a request to a protected endpoint has no Authorization header
- **THEN** the system returns HTTP 401

#### Scenario: Expired token rejected
- **WHEN** a request includes an expired JWT token
- **THEN** the system returns HTTP 401
