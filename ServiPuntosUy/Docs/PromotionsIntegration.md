# Integración de Promociones en Compras y Canjes

Este documento explica cómo funciona el sistema de promociones en ServiPuntosUy y cómo integrarlo desde el frontend.

## Descripción General

El sistema permite aplicar promociones (descuentos) tanto a compras como a canjes de productos. Cuando un producto está en oferta para una sucursal específica o para todo el tenant, el sistema automáticamente:

1. Detecta la promoción aplicable
2. Aplica el precio promocional
3. Registra la información de la promoción en la transacción
4. Calcula correctamente los puntos ganados/gastados basados en los precios promocionales

## Modelos Relevantes

- **Promotion**: Define una promoción con un precio especial para ciertos productos
- **PromotionBranch**: Asocia una promoción a una sucursal específica
- **PromotionProduct**: Asocia una promoción a un producto específico
- **TransactionItem**: Incluye campos para registrar el precio original y la promoción aplicada

## Flujo de Aplicación de Promociones

### En Compras

1. El usuario selecciona productos para comprar
2. El sistema verifica si hay promociones aplicables para cada producto en la sucursal seleccionada
3. Si existe una promoción, se aplica el precio promocional
4. Se registra tanto el precio original como el precio promocional en el item de la transacción
5. Los puntos se calculan basados en el precio promocional

### En Canjes

1. El usuario selecciona un producto para canjear
2. El sistema verifica si hay promociones aplicables para el producto en la sucursal seleccionada
3. Si existe una promoción, se aplica el precio promocional para calcular los puntos necesarios
4. El token de canje incluye información sobre la promoción aplicada
5. Al procesar el canje, se verifica que la promoción siga siendo válida

## Ejemplos de Integración Frontend

### Listado de Productos con Promociones

**Endpoint**: `GET /api/products`

**Respuesta**:
```json
[
  {
    "id": 1,
    "name": "Producto A",
    "description": "Descripción del producto A",
    "price": 100.00,
    "imageUrl": "https://example.com/productA.jpg",
    "ageRestricted": false
  },
  {
    "id": 2,
    "name": "Producto B",
    "description": "Descripción del producto B",
    "price": 200.00,
    "imageUrl": "https://example.com/productB.jpg",
    "ageRestricted": true
  }
]
```

Para mostrar si un producto tiene promoción, debes consultar el endpoint de promociones:

**Endpoint**: `GET /api/promotions`

**Respuesta**:
```json
[
  {
    "promotionId": 1,
    "tenantId": 1,
    "description": "Oferta especial",
    "startDate": "2025-06-01T00:00:00Z",
    "endDate": "2025-07-01T00:00:00Z",
    "branches": [1, 2],
    "products": [1],
    "price": 80.00
  }
]
```

En este ejemplo, el Producto A (id: 1) tiene una promoción que reduce su precio de 100.00 a 80.00 en las sucursales 1 y 2.

### Crear una Transacción (Compra)

**Endpoint**: `POST /api/transactions`

**Solicitud**:
```json
{
  "branchId": 1,
  "products": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 2,
      "quantity": 1
    }
  ]
}
```

**Respuesta**:
```json
{
  "id": 123,
  "userId": 456,
  "branchId": 1,
  "amount": 360.00,
  "pointsEarned": 36,
  "pointsSpent": 0,
  "type": "Purchase",
  "createdAt": "2025-06-20T21:30:00Z",
  "items": [
    {
      "id": 1,
      "transactionId": 123,
      "productId": 1,
      "quantity": 2,
      "unitPrice": 80.00,
      "originalPrice": 100.00,
      "promotionId": 1,
      "productName": "Producto A",
      "productImageUrl": "https://example.com/productA.jpg",
      "promotionDescription": "Oferta especial",
      "hasPromotion": true,
      "discount": 20.00,
      "discountPercentage": 20.00
    },
    {
      "id": 2,
      "transactionId": 123,
      "productId": 2,
      "quantity": 1,
      "unitPrice": 200.00,
      "originalPrice": 200.00,
      "promotionId": null,
      "productName": "Producto B",
      "productImageUrl": "https://example.com/productB.jpg",
      "promotionDescription": null,
      "hasPromotion": false,
      "discount": 0.00,
      "discountPercentage": 0.00
    }
  ],
  "hasPromotions": true,
  "totalDiscount": 40.00,
  "originalAmount": 400.00
}
```

En este ejemplo, el Producto A tiene una promoción aplicada, mientras que el Producto B no. La respuesta incluye información detallada sobre los descuentos aplicados.

### Generar Token de Canje

**Endpoint**: `POST /api/redemptions/token`

**Solicitud**:
```json
{
  "branchId": 1,
  "productId": 1
}
```

**Respuesta**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

El token incluye internamente la información sobre la promoción aplicada, si existe.

### Procesar Canje

**Endpoint**: `POST /api/redemptions/process`

**Solicitud**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Respuesta**:
```json
{
  "id": 124,
  "userId": 456,
  "branchId": 1,
  "amount": 0.00,
  "pointsEarned": 0,
  "pointsSpent": 8,
  "type": "Redemption",
  "createdAt": "2025-06-20T21:35:00Z",
  "items": [
    {
      "id": 3,
      "transactionId": 124,
      "productId": 1,
      "quantity": 1,
      "unitPrice": 80.00,
      "originalPrice": 100.00,
      "promotionId": 1,
      "productName": "Producto A",
      "productImageUrl": "https://example.com/productA.jpg",
      "promotionDescription": "Oferta especial",
      "hasPromotion": true,
      "discount": 20.00,
      "discountPercentage": 20.00
    }
  ],
  "hasPromotions": true,
  "totalDiscount": 20.00,
  "originalAmount": 100.00
}
```

## Visualización en el Frontend

### Mostrar Productos con Promociones

```html
<div class="product-card">
  <img src="{{product.imageUrl}}" alt="{{product.name}}">
  <h3>{{product.name}}</h3>
  <p>{{product.description}}</p>
  
  <!-- Si el producto tiene promoción -->
  <div v-if="hasPromotion(product.id)" class="promotion-badge">
    <span class="original-price">{{formatCurrency(product.price)}}</span>
    <span class="promotion-price">{{formatCurrency(getPromotionPrice(product.id))}}</span>
    <span class="discount-percentage">{{getDiscountPercentage(product.id)}}% OFF</span>
  </div>
  
  <!-- Si no tiene promoción -->
  <div v-else class="regular-price">
    {{formatCurrency(product.price)}}
  </div>
  
  <button @click="addToCart(product)">Agregar al carrito</button>
</div>
```

### Mostrar Resumen de Compra

```html
<div class="cart-summary">
  <h2>Resumen de compra</h2>
  
  <div v-for="item in cartItems" class="cart-item">
    <div class="item-details">
      <img :src="item.product.imageUrl" :alt="item.product.name">
      <div>
        <h3>{{item.product.name}}</h3>
        <p>Cantidad: {{item.quantity}}</p>
      </div>
    </div>
    
    <div class="item-price">
      <!-- Si el producto tiene promoción -->
      <div v-if="hasPromotion(item.product.id)" class="promotion-price">
        <span class="original-price">{{formatCurrency(item.product.price * item.quantity)}}</span>
        <span class="final-price">{{formatCurrency(getPromotionPrice(item.product.id) * item.quantity)}}</span>
        <span class="discount-amount">Ahorro: {{formatCurrency(getDiscountAmount(item.product.id) * item.quantity)}}</span>
      </div>
      
      <!-- Si no tiene promoción -->
      <div v-else class="regular-price">
        {{formatCurrency(item.product.price * item.quantity)}}
      </div>
    </div>
  </div>
  
  <div class="cart-totals">
    <div v-if="hasAnyPromotion" class="promotion-summary">
      <div class="original-total">
        <span>Subtotal:</span>
        <span>{{formatCurrency(originalTotal)}}</span>
      </div>
      <div class="discount-total">
        <span>Descuentos:</span>
        <span>-{{formatCurrency(totalDiscount)}}</span>
      </div>
    </div>
    
    <div class="final-total">
      <span>Total:</span>
      <span>{{formatCurrency(finalTotal)}}</span>
    </div>
    
    <div class="points-earned">
      <span>Puntos a ganar:</span>
      <span>{{calculatePointsToEarn()}}</span>
    </div>
  </div>
  
  <button @click="checkout">Finalizar compra</button>
</div>
```

## Consideraciones Importantes

1. **Validación de fechas**: Las promociones tienen fechas de inicio y fin. El sistema automáticamente verifica que la promoción esté vigente.

2. **Múltiples promociones**: Si un producto tiene múltiples promociones aplicables, el sistema selecciona la que ofrece el precio más bajo.

3. **Promociones a nivel de tenant vs. sucursal**: Las promociones pueden aplicarse a todas las sucursales de un tenant o solo a sucursales específicas.

4. **Cálculo de puntos**: Los puntos se calculan basados en el precio final después de aplicar las promociones.

5. **Expiración de promociones**: Si una promoción expira entre la generación del token de canje y el procesamiento del canje, se utilizará el precio regular.

## Errores Comunes

- **Promoción no encontrada**: Asegúrate de que la promoción esté asociada tanto al producto como a la sucursal.
- **Promoción expirada**: Verifica las fechas de inicio y fin de la promoción.
- **Stock insuficiente**: Aunque haya una promoción, el producto debe tener stock disponible.
- **Puntos insuficientes**: Para canjes, el usuario debe tener suficientes puntos incluso con el precio promocional.
