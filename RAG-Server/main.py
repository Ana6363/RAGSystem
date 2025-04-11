from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from query_engine import query_pdf
from dotenv import load_dotenv
import os

load_dotenv()

print("Model:", os.getenv("OPENAI_MODEL"))

app = FastAPI()

class QueryRequest(BaseModel):
    file_id: str
    question: str

@app.post("/query")
async def query(request: QueryRequest):
    try:
        response = query_pdf(request.file_id, request.question)
        return {"answer": response}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
