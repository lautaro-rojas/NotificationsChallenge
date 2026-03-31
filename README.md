# NotificationsChallenge
Notification management system for authenticated users.  The system allows each user to manage and send notifications through various channels, such as: Email, SMS, Push Notifications

# Tecnologies
- C#
- ASP.NET Core WebAPi
- Swagger -> Scalar
- Docker
- JWT
- Entity Framework 
- Clean Architecture

es
# Documentación de la API
## Transición de Swagger a Scalar
Históricamente, el ecosistema .NET ha utilizado Swashbuckle (Swagger UI) como la herramienta predeterminada para explorar y probar APIs. Sin embargo, a partir de .NET 9 y consolidado en nuestra actual versión .NET 10, Microsoft retiró Swashbuckle de sus plantillas oficiales, impulsando una solución más moderna y con soporte oficial a largo plazo.

Para alinearnos con las mejores prácticas y los estándares actuales de la industria, hemos implementado el sistema nativo Microsoft.AspNetCore.OpenApi para la generación del documento (JSON), y adoptado Scalar como nuestra interfaz gráfica interactiva.

### ¿Por qué hicimos este cambio?
- Arquitectura Nativa: La especificación OpenAPI ahora es generada directamente por el framework de .NET, eliminando la dependencia de librerías de terceros obsoletas para el core de la documentación.
- Experiencia de Desarrollo Superior: Scalar ofrece una interfaz visual mucho más limpia, rápida y moderna. Su diseño se asemeja a herramientas profesionales como Postman o ReDoc, dejando atrás la interfaz anticuada de Swagger.
- Prueba de Seguridad Simplificada (JWT): Scalar maneja la inyección de esquemas de seguridad y tokens Bearer de forma mucho más intuitiva e integrada para el desarrollador.
- Future-Proofing: Nos aseguramos de que el proyecto utilice paquetes con mantenimiento activo y listos para las futuras iteraciones de .NET.