import os
from dotenv import load_dotenv
from langchain_community.document_loaders import PyPDFLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_openai import OpenAIEmbeddings
from langchain_community.vectorstores import Qdrant

load_dotenv()

QDRANT_URL = os.getenv("QDRANT_URL")
QDRANT_API_KEY = os.getenv("QDRANT_API_KEY")
QDRANT_COLLECTION = os.getenv("QDRANT_COLLECTION", "nbanks_documents")
PDF_FOLDER = os.getenv("PDF_FOLDER", "./pdfs")
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")


def embed_and_store(file_id: str):
    file_path = os.path.join(PDF_FOLDER, f"{file_id}.pdf") # Construct the full path to the PDF file(file_id + PDF_FOLDER from .env)
    
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"File not found: {file_path}")

    # Load the PDF file - this will read the content of the PDF and convert it into a format that can be processed by LangChain.
    # The loader will extract text from the PDF and create a list of documents.
    loader = PyPDFLoader(file_path)
    docs = loader.load()

    # Split the text into manageable chunks
    # 100 characters of overlap between adjacent chunks
    text_splitter = RecursiveCharacterTextSplitter(chunk_size=500, chunk_overlap=100)
    chunks = text_splitter.split_documents(docs)

    # Adds file_id to each chunk so later we can filter search results to just this PDF
    for chunk in chunks:
        chunk.metadata["file_id"] = file_id

    print(f"📄 Split PDF into {len(chunks)} chunks")

    # Creates embeddings (vectors) for the text using OpenAI’s model
    embeddings = OpenAIEmbeddings(api_key=OPENAI_API_KEY)

    # Sends the chunks + their vectors to Qdrant Cloud collection.
    # They’re now stored, searchable, and ready to be queried later!
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
    embed_and_store("mock_document_2")
    embed_and_store("mock_document_3")
