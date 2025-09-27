# Pages/Contact.cshtml & Contact.cshtml.cs

## Contact.cshtml
A user information collection form that includes:
- Full Name (required)
- Email Address (required, with email validation)
- Phone Number (optional, with phone format validation)
- Message (optional, up to 500 characters)

The form uses Bootstrap styling for responsive design and includes client-side validation.

## Contact.cshtml.cs
Implements the page model for the contact form with:
- Form validation attributes using Data Annotations
- POST-REDIRECT-GET pattern for form submission
- Success message display using TempData
- Logging of form submissions
- ContactForm class containing all form fields with appropriate validation

## Features
- Required field validation for Name and Email
- Email format validation
- Phone number format validation (optional field)
- Message length validation (max 500 characters)
- Success message after form submission
- Form clearing after successful submission
- Responsive Bootstrap form layout