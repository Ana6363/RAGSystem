import os
from dotenv import load_dotenv
from langchain_community.document_loaders import PyPDFLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_openai import OpenAIEmbeddings
from langchain_community.vectorstores import Qdrant

# Load environment variables
load_dotenv()

QDRANT_URL = os.getenv("QDRANT_URL")
QDRANT_API_KEY = os.getenv("QDRANT_API_KEY")
QDRANT_COLLECTION = os.getenv("QDRANT_COLLECTION", "nbanks_documents")
PDF_FOLDER = os.getenv("PDF_FOLDER", "./pdfs")

OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")

def embed_and_store(file_id: str):
    file_path = os.path.join(PDF_FOLDER, f"{file_id}.pdf")
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"❌ File not found: {file_path}")

    # 1. Load the PDF
    loader = PyPDFLoader(file_path)
    docs = loader.load()

    # 2. Split into chunks
    text_splitter = RecursiveCharacterTextSplitter(chunk_size=500, chunk_overlap=100)
    chunks = text_splitter.split_documents(docs)

    # After splitting
    for chunk in chunks:
        chunk.metadata["file_id"] = file_id

    print(f"📄 Split PDF into {len(chunks)} chunks")

    # 3. Generate embeddings
    embeddings = OpenAIEmbeddings(api_key=OPENAI_API_KEY)

    # 4. Upload to Qdrant Cloud
    vectorstore = Qdrant.from_documents(
        documents=chunks,
        embedding=embeddings,
        url=QDRANT_URL,
        api_key=QDRANT_API_KEY,
        collection_name=QDRANT_COLLECTION,
    )

    print(f"✅ Embedded and stored {len(chunks)} chunks in Qdrant Cloud!")


# Example usage
if __name__ == "__main__":
    embed_and_store("mock_document_1")
