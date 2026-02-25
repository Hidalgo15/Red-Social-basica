# Red-Social-basica
Proyecto de una Red Social Básica "BookFace", funcionalidades basicas

BookFace es una red social básica desarrollada con el ecosistema .NET, diseñada bajo los principios de Clean Architecture (Onion Architecture). El objetivo principal de este proyecto es demostrar el uso de patrones de diseño, desacoplamiento de código y persistencia de datos profesional.

El proyecto sigue el patrón de Onion Architecture, lo que permite que la lógica de negocio sea independiente de los frameworks y las herramientas externas.

<img width="389" height="310" alt="image" src="https://github.com/user-attachments/assets/2c8812c8-f640-4b02-94cf-18738b297c72" />


Estructura de Capas:
Core (Domain & Application): Contiene las entidades de negocio, interfaces de repositorio, DTOs y la lógica de servicios. Es el corazón de la aplicación y no tiene dependencias externas.


<img width="425" height="513" alt="image" src="https://github.com/user-attachments/assets/344f9ed2-b2ff-4690-9631-5fb50109d366" />

Infrastructure (Persistence, Identity & Shared): * Persistence: Implementación de Entity Framework Core con un enfoque Code-First.
Identity: Gestión de usuarios y seguridad utilizando ASP.NET Core Identity.
Shared: Servicios transversales como el envío de correos (SMTP).


<img width="421" height="485" alt="image" src="https://github.com/user-attachments/assets/a8e33bb7-c11f-4cb3-b7a3-96a50eca89c3" />

Presentation (WebApp): La interfaz de usuario desarrollada en ASP.NET Core MVC que consume los servicios de la capa de aplicación


<img width="311" height="330" alt="image" src="https://github.com/user-attachments/assets/0d3f20fb-6c75-4751-a51d-7ec69580c052" />
