import os
from langchain_community.document_loaders import PyPDFLoader
from langchain.chains.question_answering import load_qa_chain
from langchain.llms import OpenAI
from dotenv import load_dotenv

load_dotenv()

OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
OPENAI_MODEL = os.getenv("OPENAI_MODEL")
PDF_FOLDER = os.getenv("PDF_FOLDER")

llm = OpenAI(temperature=0, openai_api_key=OPENAI_API_KEY)


def query_pdf(file_id: str, question: str) -> str:
    file_path = os.path.join(PDF_FOLDER, file_id) # Construct the full path to the PDF file(file_id + PDF_FOLDER from .env)

    # Check if the file exists
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"❌ File not found: {file_path}")

    loader = PyPDFLoader(file_path)

    # Load the PDF file - this will read the content of the PDF and convert it into a format that can be processed by LangChain.
    # The loader will extract text from the PDF and create a list of documents.
    docs = loader.load()

    # Build the prompt for the LLM
    chain = load_qa_chain(llm, chain_type="stuff") # Loads a prebuilt chain template for question answering.
    result = chain.run(input_documents=docs, question=question) # Runs the chain with the loaded documents and the user's question.

    return result
