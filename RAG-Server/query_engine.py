import os
from langchain_community.document_loaders import PyPDFLoader
from langchain.chains.question_answering import load_qa_chain
from langchain_openai import OpenAI
from dotenv import load_dotenv
from langchain_community.vectorstores import Qdrant
from langchain_openai import OpenAIEmbeddings

load_dotenv()

OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
OPENAI_MODEL = os.getenv("OPENAI_MODEL")
PDF_FOLDER = os.getenv("PDF_FOLDER")
QDRANT_URL = os.getenv("QDRANT_URL")
QDRANT_API_KEY = os.getenv("QDRANT_API_KEY")
QDRANT_COLLECTION = os.getenv("QDRANT_COLLECTION", "nbanks_documents")
from langchain_qdrant import Qdrant
from qdrant_client import QdrantClient

llm = OpenAI(temperature=0, openai_api_key=OPENAI_API_KEY)

embeddings = OpenAIEmbeddings(api_key=OPENAI_API_KEY)

qdrant_client = QdrantClient(
    url=QDRANT_URL,
    api_key=QDRANT_API_KEY,
)

vectorstore = Qdrant(
    client=qdrant_client,
    collection_name=QDRANT_COLLECTION,
    embeddings=embeddings,
)


def query_pdf(file_id: str, question: str) -> str:
    # Retrieve only chunks for this file_id
    relevant_docs = vectorstore.similarity_search(
        query=question,
        k=4,
        filter={"file_id": file_id}
    )

    if not relevant_docs:
        raise ValueError(f"No chunks found for file_id: {file_id}")

    chain = load_qa_chain(llm, chain_type="stuff")
    result = chain.run(input_documents=relevant_docs, question=question)
    return result