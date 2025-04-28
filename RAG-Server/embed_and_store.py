import os
from dotenv import load_dotenv
from langchain_community.document_loaders import PyPDFLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_openai import OpenAIEmbeddings
from langchain_community.vectorstores import Qdrant
from pymongo import MongoClient
import sys
from bson import ObjectId
from langchain.schema import Document as LangDocument


load_dotenv()

# MongoDB
MONGO_URI = os.getenv("MONGO_URI")
DB_NAME = os.getenv("MONGO_DB_NAME", "nBanksUsers")
PDF_FOLDER = os.getenv("PDF_FOLDER", "./temp_pdfs")

# Qdrant
QDRANT_URL = os.getenv("QDRANT_URL")
QDRANT_API_KEY = os.getenv("QDRANT_API_KEY")
QDRANT_COLLECTION = os.getenv("QDRANT_COLLECTION", "nbanks_documents")
COLLECTION_NAME = "Documents"

# OpenAI
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")

def embed_and_store(file_id: str):
    # Connect to MongoDB
    client = MongoClient(MONGO_URI)
    db = client[DB_NAME]
    documents_collection = db[COLLECTION_NAME]

    # Find document by file_id
    document = documents_collection.find_one({"_id": ObjectId(file_id)})
    if not document:
        raise FileNotFoundError(f"No document found in MongoDB with id: {file_id}")

    print(f"✅ Found document with ID {file_id} in MongoDB.")

    # Get the extracted text (raw)
    text_content = document["content"]["ContentValue"]

    # Create a LangChain Document from text
    lang_doc = LangDocument(page_content=text_content, metadata={"file_id": file_id})

    # Split the text into manageable chunks
    text_splitter = RecursiveCharacterTextSplitter(chunk_size=500, chunk_overlap=100)
    chunks = text_splitter.split_documents([lang_doc])

    print(f"📄 Split text into {len(chunks)} chunks")

    # Create embeddings
    embeddings = OpenAIEmbeddings(api_key=OPENAI_API_KEY)

    # Upload to Qdrant
    vectorstore = Qdrant.from_documents(
        documents=chunks,
        embedding=embeddings,
        url=QDRANT_URL,
        api_key=QDRANT_API_KEY,
        collection_name=QDRANT_COLLECTION,
    )

    print(f"✅ Embedded and stored {len(chunks)} chunks in Qdrant Cloud!")

# Entry point when called from .NET
if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python embed_and_store.py <file_id>")
        sys.exit(1)

    file_id = sys.argv[1]
    embed_and_store(file_id)
