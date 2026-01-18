FROM python:3.11-slim

WORKDIR /app

# Install dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy application code
COPY ./app ./app
COPY ./alembic ./alembic
COPY ./alembic.ini ./alembic.ini

# Copy entrypoint script
COPY ./entrypoint.sh .
# Fix windows line endings and make executable
RUN sed -i 's/\r$//' entrypoint.sh && chmod +x entrypoint.sh

# Run the application
ENTRYPOINT ["./entrypoint.sh"]
