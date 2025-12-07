# Frontend Fuel System - Configuración para Deployment

## Variables de Entorno

Para que la aplicación funcione correctamente en producción, necesitas configurar las variables de entorno:

### Desarrollo Local
Archivo: `.env`
```
REACT_APP_API_BASE_URL=http://localhost:5011/api
```

### Producción 
Archivo: `.env.production` (o configurar en tu plataforma de hosting)
```
REACT_APP_API_BASE_URL=https://tu-backend-url.com/api
```

## Platforms de Hosting

### Azure Static Web Apps
1. Configura la variable en el portal de Azure:
   - Ve a tu Static Web App
   - Configuration → Environment variables
   - Añade: `REACT_APP_API_BASE_URL` = `https://tu-backend-url.com/api`

### Netlify
1. En el dashboard de Netlify:
   - Site settings → Environment variables
   - Añade: `REACT_APP_API_BASE_URL` = `https://tu-backend-url.com/api`

### Vercel
1. En el dashboard de Vercel:
   - Settings → Environment Variables
   - Añade: `REACT_APP_API_BASE_URL` = `https://tu-backend-url.com/api`

### Heroku
```bash
heroku config:set REACT_APP_API_BASE_URL=https://tu-backend-url.com/api
```

## Scripts de Build

```bash
# Desarrollo
npm start

# Build para producción
npm run build

# Test
npm test
```

## Verificación

Para verificar que las variables están funcionando:
1. Abre las Developer Tools en el navegador
2. Ve a Network tab
3. Realiza login o cualquier acción que haga llamadas API
4. Verifica que las URLs apunten a tu backend de producción

## Troubleshooting

- **Error de CORS**: Asegúrate que tu backend permita requests del dominio donde está hosteado tu frontend
- **404 en API calls**: Verifica que `REACT_APP_API_BASE_URL` esté configurado correctamente
- **Variable no se aplica**: Las variables de entorno deben empezar con `REACT_APP_` en React