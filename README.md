# Merch App

**A Windows desktop application for merch rental management, built for Stop Domestic Abuse (SDA).**

---

## 🇬🇧 
### What is Merch App?

Merch App is a WinUI 3 desktop application that allows organisations to manage the rental of promotional materials (merch) internally. It was built for **Stop Domestic Abuse**, a UK-based charity, as part of their internal tooling to track who has borrowed what, when items are due back, and to streamline the approval process.

The application uses **Microsoft 365 / SharePoint Online** as its backend — no separate database or server is required. Authentication is handled via **Azure Active Directory**, and email notifications are sent via **Power Automate**.

---

### Key Features

**For Users:**
- Browse a catalogue of available merch items
- Select items using checkboxes and add them to a request
- Choose rental dates using an interactive calendar
- Add a purpose/note to each request
- Submit requests directly to the manager for approval
- Track the status of all rental requests (Pending, Approved, Rejected, Returned, Overdue)
- Receive email notifications when requests are approved or rejected
- View in-app notifications

**For Managers:**
- View all rental requests in one place
- Expand individual requests to see full details (items, dates, purpose)
- Bulk approve or reject multiple pending requests at once
- Add a reason when rejecting requests (visible to the user)
- Mark approved rentals as returned
- Filter requests by status (All, Pending, Active, Overdue, Returned)
- Receive email notifications when new requests are submitted

---

### Tech Stack

| Technology | Purpose |
|---|---|
| WinUI 3 / Windows App SDK | UI framework |
| C# / .NET 10 | Application logic |
| SharePoint Online (CSOM) | Data storage |
| Azure Active Directory (MSAL) | Authentication |
| Power Automate | Email notifications |
| CommunityToolkit.Mvvm | MVVM pattern |
| Microsoft.Identity.Client | OAuth2 login |

---

### Architecture

```
┌─────────────────────────────────────────┐
│           Merch App (WinUI 3)           │
│                                         │
│  Views → ViewModels → Services          │
│                  ↓                      │
│         SharePoint Online               │
│    (MerchItems, RentalRequests,         │
│         RentalItems lists)              │
│                  ↓                      │
│   Azure AD ←→ MSAL Authentication       │
│                  ↓                      │
│    Power Automate → Email (Outlook)     │
└─────────────────────────────────────────┘
```

---

### Setup

See the full [Setup Guide](SETUP.md) for step-by-step instructions on:
- Creating a SharePoint site and lists
- Registering the app in Azure Active Directory
- Configuring Power Automate flows
- Setting up user roles
- Building and deploying the application

---

### Configuration

Before running the app, edit `Config/appsettings.json`:

```json
{
  "SharePoint": {
    "SiteUrl": "https://YOUR_TENANT.sharepoint.com/sites/MerchApp",
    "ClientId": "YOUR_AZURE_APP_CLIENT_ID",
    "TenantId": "YOUR_AZURE_TENANT_ID",
    "ItemsListName": "MerchItems",
    "RentalRequestsListName": "RentalRequests",
    "RentalItemsListName": "RentalItems"
  },
  "Roles": {
    "ManagerEmail": "manager@yourdomain.com"
  }
}
```

---

### Project Structure

```
MerchApp/
├── Models/              # Data models (Item, RentalRequest, AppUser, etc.)
├── Services/            # Business logic and SharePoint/Auth services
│   └── Interfaces/      # Service interfaces (SOLID principles)
├── ViewModels/          # MVVM ViewModels
├── Views/               # XAML pages
├── Converters/          # XAML value converters
├── Config/              # appsettings.json
└── Assets/              # Icons and images
```

---

### Screenshots

> Login page with SDA branding, dark theme

> Catalogue with checkbox selection and cart

> Manager view with bulk approve/reject

> My Rentals with status tracking

---

### About

Built by **Paweł Szymczyk** ([Pawel Szymczyk IT Services](https://github.com/Pawel-Szymczyk)) as a freelance MVP for Stop Domestic Abuse.

---
---

## 🇵🇱 
### Czym jest Merch App?

Merch App to aplikacja desktopowa WinUI 3, która umożliwia organizacjom zarządzanie wypożyczaniem materiałów promocyjnych (merch) wewnętrznie. Została zbudowana dla **Stop Domestic Abuse** — brytyjskiej organizacji charytatywnej — jako narzędzie do śledzenia kto, co i kiedy pożyczył, kiedy przedmioty mają zostać zwrócone, oraz do usprawnienia procesu zatwierdzania.

Aplikacja używa **Microsoft 365 / SharePoint Online** jako backendu — nie wymaga oddzielnej bazy danych ani serwera. Uwierzytelnianie odbywa się przez **Azure Active Directory**, a powiadomienia emailowe są wysyłane przez **Power Automate**.

---

### Główne funkcje

**Dla użytkowników:**
- Przeglądanie katalogu dostępnych przedmiotów merch
- Wybieranie przedmiotów za pomocą checkboxów i dodawanie ich do prośby
- Wybór dat wypożyczenia za pomocą interaktywnego kalendarza
- Dodawanie celu/notatki do każdej prośby
- Wysyłanie próśb do managera w celu zatwierdzenia
- Śledzenie statusu wszystkich próśb o wypożyczenie (Oczekująca, Zatwierdzona, Odrzucona, Zwrócona, Po terminie)
- Otrzymywanie powiadomień email gdy prośby są zatwierdzone lub odrzucone
- Przeglądanie powiadomień w aplikacji

**Dla managerów:**
- Widok wszystkich próśb o wypożyczenie w jednym miejscu
- Rozwijanie poszczególnych próśb aby zobaczyć pełne szczegóły (przedmioty, daty, cel)
- Hurtowe zatwierdzanie lub odrzucanie wielu oczekujących próśb jednocześnie
- Dodawanie powodu przy odrzucaniu próśb (widoczny dla użytkownika)
- Oznaczanie zatwierdzonych wypożyczeń jako zwrócone
- Filtrowanie próśb według statusu (Wszystkie, Oczekujące, Aktywne, Po terminie, Zwrócone)
- Otrzymywanie powiadomień email gdy nowe prośby są składane

---

### Stos technologiczny

| Technologia | Przeznaczenie |
|---|---|
| WinUI 3 / Windows App SDK | Framework UI |
| C# / .NET 10 | Logika aplikacji |
| SharePoint Online (CSOM) | Przechowywanie danych |
| Azure Active Directory (MSAL) | Uwierzytelnianie |
| Power Automate | Powiadomienia email |
| CommunityToolkit.Mvvm | Wzorzec MVVM |
| Microsoft.Identity.Client | Logowanie OAuth2 |

---

### Konfiguracja

Przed uruchomieniem aplikacji edytuj `Config/appsettings.json`:

```json
{
  "SharePoint": {
    "SiteUrl": "https://TWOJ_TENANT.sharepoint.com/sites/MerchApp",
    "ClientId": "TWOJ_AZURE_APP_CLIENT_ID",
    "TenantId": "TWOJ_AZURE_TENANT_ID",
    "ItemsListName": "MerchItems",
    "RentalRequestsListName": "RentalRequests",
    "RentalItemsListName": "RentalItems"
  },
  "Roles": {
    "ManagerEmail": "manager@twojadomena.com"
  }
}
```

---

### O projekcie

Zbudowane przez **Pawła Szymczyka** ([Pawel Szymczyk IT Services](https://github.com/Pawel-Szymczyk)) jako freelance MVP dla Stop Domestic Abuse.
