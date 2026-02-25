# Red-Social-basica
Proyecto de una Red Social Básica "BookFace", funcionalidades basicas

BookFace es una red social básica desarrollada con el ecosistema .NET, diseñada bajo los principios de Clean Architecture (Onion Architecture). El objetivo principal de este proyecto es demostrar el uso de patrones de diseño, desacoplamiento de código y persistencia de datos profesional.

El proyecto sigue el patrón de Onion Architecture, lo que permite que la lógica de negocio sea independiente de los frameworks y las herramientas externas.

<img width="411" height="335" alt="image" src="https://github.com/user-attachments/assets/0e2a2308-746b-42fd-80d4-46d5c36e7be4" />

Estructura de Capas:
Core (Domain & Application): Contiene las entidades de negocio, interfaces de repositorio, DTOs y la lógica de servicios. Es el corazón de la aplicación y no tiene dependencias externas.


<img width="370" height="482" alt="image" src="https://github.com/user-attachments/assets/bada3c6b-bd2b-43dc-ba96-7a2ba2205897" />

Infrastructure (Persistence, Identity & Shared): * Persistence: Implementación de Entity Framework Core con un enfoque Code-First.
Identity: Gestión de usuarios y seguridad utilizando ASP.NET Core Identity.
Shared: Servicios transversales como el envío de correos (SMTP).


<img width="428" height="484" alt="image" src="https://github.com/user-attachments/assets/a2decde0-6449-4b96-a2a2-4a9a6c7e0a49" />

Presentation (WebApp): La interfaz de usuario desarrollada en ASP.NET Core MVC que consume los servicios de la capa de aplicación


<img width="295" height="333" alt="image" src="https://github.com/user-attachments/assets/0aef2316-81d1-4390-befd-9f0065cef886" />
