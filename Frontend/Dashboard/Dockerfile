# Stage 1: Build the Angular application
FROM node:latest AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
RUN npm install -g @angular/cli
COPY . .

RUN npm run build --configuration=production

# stage 2: 
FROM nginx:latest

# copy the build output to replace the default nginx contents.
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/demo/browser /usr/share/nginx/html

EXPOSE 80



# build: docker build -t Dashboard