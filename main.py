from dotenv import load_dotenv
load_dotenv()

from typing import Union
from fastapi import FastAPI

from routes import adminRouter, authRouter

app = FastAPI()

@app.get("/")
def read_root():
    return {"Hello": "World"}

app.include_router(adminRouter)
app.include_router(authRouter)