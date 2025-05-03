from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from query_engine import query_pdf
from dotenv import load_dotenv
from embed_and_store import embed_and_store
from fastapi import Query
from typing import List
from query_engine import vectorstore, llm
from langchain.chains.question_answering import load_qa_chain

import os

load_dotenv()

app = FastAPI(debug=True)

#Defines de expected format of the resquest body.
class QueryRequest(BaseModel):
    file_ids: List[str]
    question: str

# FastAPI route - entry point for when "user sends a request".
# When someone sends a POST request to the /query endpoint with a body that matches QueryRequest,
# the query function will be called.
@app.post("/query")
async def query(request: QueryRequest):
    try:
        relevant_docs = []

        for file_id in request.file_ids:
            docs = vectorstore.similarity_search(
                query=request.question,
                k=4,
                filter={"file_id": file_id}
            )
            relevant_docs.extend(docs)

        if not relevant_docs:
            raise ValueError("No relevant documents found for any file_id.")

        chain = load_qa_chain(llm, chain_type="stuff")

        result = chain.run(input_documents=relevant_docs, question=request.question)

        return {"answer": result}

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


@app.post("/embed")
async def embed(file_id: str = Query(...)):
    try:
        embed_and_store(file_id)
        return {"status": "embedding_completed"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
