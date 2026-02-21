# Stage 1: Build the Angular application
FROM node:22 AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
RUN npm install -g @angular/cli
COPY . .

RUN npm run build -- --configuration=production

# stage 2: 
FROM nginx:alpine

# copy the build output to replace the default nginx contents.
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/Dashboard/browser /usr/share/nginx/html

EXPOSE 80

# run nginx
CMD ["nginx", "-g", "daemon off;"]


# build: docker build -t frontend . 
# build: docker build --no-cache -t frontend .
# run: docker run -d -p 4200:80 --name dashboard frontend:1.0
