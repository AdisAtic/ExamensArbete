<!-- PROJECT SHIELDS -->
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](#)
[![NuGet](https://img.shields.io/nuget/v/PriceTrackingAPI.svg)](#)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

# PriceTrackingAPI

Ett .NET 9.0–baserat RESTful Web API för att spåra och hämta prisdata från olika källor.  
API:t exponerar endpoints för att:
- Lägga till och hantera produkter  
- Hämta historiska prisvärden  
- Konfigurera bevakningar och notifieringar vid prisförändringar  

---

## 📖 Innehållsförteckning

1. [Om projektet](#om-projektet)  
2. [Förutsättningar](#förutsättningar)  
3. [Installation](#installation)  
4. [Användning](#användning)  
5. [API-endpoints](#api-endpoints)  
6. [Beroenden](#beroenden)  
7. [Transitiva paket](#transitiva-paket)  
8. [Konfiguration](#konfiguration)  
9. [Kör tester](#kör-tester)  
10. [Deployment](#deployment)  
11. [Bidra](#bidra)  
12. [Versionshantering](#versionshantering)  
13. [Licens](#licens)  
14. [Kontakt](#kontakt)  

---

## 📋 Om projektet

**PriceTrackingAPI** hjälper dig att automatisera insamling av prisdata och hålla koll på prisförändringar över tid. Använd det för:

- Övervakning av produktpriser på e-handelsplattformar  
- Dataanalys och trendspaning  
- Notifieringar vid prisfall eller prisökning  

---

## 🔨 Förutsättningar

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)  
- En PostgreSQL-databas (eller annan EF-Core-provider)  
- (Valfritt) Docker för containeriserad utveckling  

---

## 🚀 Installation

1. **Klona** repot  
   ```bash
   git clone https://github.com/ditt-anvandarnamn/PriceTrackingAPI.git
   cd PriceTrackingAPI
