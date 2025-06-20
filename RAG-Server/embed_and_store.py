import os
from dotenv import load_dotenv
from langchain_community.document_loaders import PyPDFLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_openai import OpenAIEmbeddings
from langchain_community.vectorstores import Qdrant
from pymongo import MongoClient
from bson import ObjectId

load_dotenv()

QDRANT_URL = os.getenv("QDRANT_URL")
QDRANT_API_KEY = os.getenv("QDRANT_API_KEY")
QDRANT_COLLECTION = os.getenv("QDRANT_COLLECTION", "nbanks_documents")
PDF_FOLDER = os.getenv("PDF_FOLDER", "./pdfs")
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
MONGO_URI = os.getenv("MONGO_URI")
MONGO_DB = os.getenv("MONGO_DB")
MONGO_COLLECTION = os.getenv("MONGO_COLLECTION")


def embed_and_store(file_id: str):
    client = MongoClient(MONGO_URI)
    db = client[MONGO_DB]
    collection = db[MONGO_COLLECTION]

    doc = collection.find_one({"_id": ObjectId(file_id)})

    if not doc:
        raise FileNotFoundError(f"No document found with _id={file_id}")

    content = doc.get("content", {}).get("ContentValue")

    if not content:
        raise ValueError(f"No content found for document with _id={file_id}")

    print(f"Loaded content from MongoDB for file_id={file_id}")

    # Turn content string into LangChain Documents
    from langchain.docstore.document import Document

    docs = [Document(page_content=content, metadata={"file_id": file_id})]

    # Split the text into manageable chunks
    text_splitter = RecursiveCharacterTextSplitter(chunk_size=500, chunk_overlap=100)
    chunks = text_splitter.split_documents(docs)

    print(f"Split content into {len(chunks)} chunks")

    # Creates embeddings
    embeddings = OpenAIEmbeddings(api_key=OPENAI_API_KEY)

    # Store chunks into Qdrant
    vectorstore = Qdrant.from_documents(
        documents=chunks,
        embedding=embeddings,
        url=QDRANT_URL,
        api_key=QDRANT_API_KEY,
        collection_name=QDRANT_COLLECTION,
    )

    print(f"Embedded and stored {len(chunks)} chunks in Qdrant Cloud!")


# Example usage
if __name__ == "__main__":
    embed_and_store("mock_document_1")
    embed_and_store("mock_document_2")
    embed_and_store("mock_document_3")