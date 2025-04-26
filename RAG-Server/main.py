from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from query_engine import query_pdf, query_pdfs
from dotenv import load_dotenv
import os

load_dotenv()

app = FastAPI()

#Defines de expected format of the resquest body.
class QueryRequest(BaseModel):
    file_id: str
    question: str

class MultiQueryRequest(BaseModel):
    file_ids: list
    question: str

# FastAPI route - entry point for when "user sends a request".
# When someone sends a POST request to the /query endpoint with a body that matches QueryRequest,
# the query function will be called.
@app.post("/query")
async def query(request: QueryRequest):
    try:
        response = query_pdf(request.file_id, request.question)
        return {"answer": response}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/multi_query")
async def multi_query(request: MultiQueryRequest):
    try:
        response = query_pdfs(request.file_ids, request.question)
        return {"answer": response}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))