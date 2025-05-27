# Guía para la utilización del Servicio de Verificación de Edad e Identidad

Este documento explica cómo utilizar el servicio de simulación de documentos digitalizados para fines educativos. El servicio genera información ficticia de personas basada en números de documentos de identidad (cédulas).

## Obtener cédulas con edades epecíficas

La edad de la persona generada se determina por **los dos últimos dígitos** del número de cédula. Ej.: para una persona de 25 años utilice una cédula que termine en 25, para una de 17 utilice una cédula que termina en 17.

La cédula `4.321.025-9` generará una persona que tiene 25 años.

## Errores y Advertencias

Además de la verificación de edad, este servicio se utiliza para verificar la identidad de una persona. Esto se lleva a cabo con los dos primeros dígitos de la cédula.

| **Dos primeros dígitos**    | **Resultado generado**                         |
| --------------------------- | ---------------------------------------------- |
| 11                          | Error - Persona inexistente                    |
| 12                          | Error - Límite de consultas excedido           |
| 13                          | Error - Número de cédula anulado               |
| 14                          | Advertencia - Datos de persona a regularizar   |
| 15                          | Advertencia - Documento hurtado o extraviado   |
| Otro                        | Sin errores ni advertencias.                   |


Para más información, dirigirse a este [link](https://git.mz.uy/marianozunino/dnic-soap/wiki/Servicio-de-Simulaci%C3%B3n-de-Documentos-Digitalizados)
