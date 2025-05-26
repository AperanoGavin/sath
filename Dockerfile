FROM python:3.9

ENV PYTHONUNBUFFERED 1

WORKDIR /app

COPY requirements.txt .

RUN pip install --no-cache-dir -r requirements.txt
RUN apt-get update && apt-get install -y libpq-dev gcc
RUN apt-get install libpq-dev -y
RUN apt-get install postgresql-client -y

COPY . .

WORKDIR /app/src

# Exposer le bon port
EXPOSE 80

CMD ["python", "manage.py", "runserver", "0.0.0.0:80"]