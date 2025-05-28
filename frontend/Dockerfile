FROM node:20

RUN npm install -g pnpm @angular/cli

WORKDIR /app

COPY . .

RUN pnpm install --force

EXPOSE 4200

#CMD ["ng", "serve", "--host", "0.0.0.0", "--port", "4200"]
CMD ["npx", "nx", "serve", "parking", "--host", "0.0.0.0", "--port", "4200"]
